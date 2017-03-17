using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PingExperiment.Attributes;
using PingExperiment.Interfaces;

namespace PingExperiment.IOC.RuntimeChecks
{
    public class NctChecks
    {
        class This { }

        public NctChecks() : this(typeof(This)) { }

        public NctChecks(Type startpoint)
            : this(startpoint.Assembly)
        { }

        public NctChecks(params Assembly[] ass)
        {
            foreach (var assembly in ass)
            {
                _assemblies.Add(assembly);

                foreach (var type in assembly.GetTypes())
                {
                    EnsureType(type);
                }
            }

            while (_typesToCheck.Count > 0)
            {
                var t = _typesToCheck.Pop();
                GatherTypesFrom(t);

                PerformRuntimeCheck(t);
            }
        }

        private readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();

        private readonly Stack<Type> _typesToCheck = new Stack<Type>();
        private readonly HashSet<Type> _typesKnown = new HashSet<Type>();

        private void EnsureType(Type t)
        {
            // Don't check for assembly here; we can pass f.ex. System.Lazy<Our.T<MyClass>>
            if (t != null && !t.IsGenericTypeDefinition && _typesKnown.Add(t))
            {
                _typesToCheck.Push(t);

                if (t.IsGenericType)
                {
                    foreach (var par in t.GetGenericArguments())
                    {
                        EnsureType(par);
                    }
                }

                if (t.IsArray)
                {
                    EnsureType(t.GetElementType());
                }
            }

        }

        private void PerformRuntimeCheck(Type t)
        {
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
            {
                // Only check the assemblies we explicitly asked for:
                if (_assemblies.Contains(t.Assembly))
                {
                    // Gather the generics data:
                    var def = t.GetGenericTypeDefinition();
                    var par = def.GetGenericArguments();
                    var args = t.GetGenericArguments();

                    // Perform checks:
                    for (var i = 0; i < args.Length; ++i)
                    {
                        foreach (var check in par[i].GetCustomAttributes(typeof(ConstraintAttribute), true).Cast<ConstraintAttribute>())
                        {
                            if (!check.Check(args[i]))
                            {
                                var error = "Runtime type check failed for type " + t + ": " + check;

                                Debugger.Break();
                                throw new ConstraintFailedException(error);
                            }
                        }
                    }
                }
            }
        }

        // Phase 1: all types that are referenced in some way
        private void GatherTypesFrom(Type t)
        {
            EnsureType(t.BaseType);

            foreach (var intf in t.GetInterfaces())
            {
                EnsureType(intf);
            }

            foreach (var nested in t.GetNestedTypes())
            {
                EnsureType(nested);
            }

            const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            foreach (var field in t.GetFields(all))
            {
                EnsureType(field.FieldType);
            }
            foreach (var property in t.GetProperties(all))
            {
                EnsureType(property.PropertyType);
            }
            foreach (var evt in t.GetEvents(all))
            {
                EnsureType(evt.EventHandlerType);
            }
            foreach (var ctor in t.GetConstructors(all))
            {
                foreach (var par in ctor.GetParameters())
                {
                    EnsureType(par.ParameterType);
                }

                // Phase 2: all types that are used in a body
                GatherTypesFrom(ctor);
            }
            foreach (var method in t.GetMethods(all))
            {
                if (method.ReturnType != typeof(void))
                {
                    EnsureType(method.ReturnType);
                }

                foreach (var par in method.GetParameters())
                {
                    EnsureType(par.ParameterType);
                }

                // Phase 2: all types that are used in a body
                GatherTypesFrom(method);
            }
        }

        private void GatherTypesFrom(MethodBase method)
        {
            if (method.DeclaringType == null || !_assemblies.Contains(method.DeclaringType.Assembly)) return;
            
            var methodBody = method.GetMethodBody();
            if (methodBody == null) return;
                
            // Handle local variables
            foreach (var local in methodBody.LocalVariables)
            {
                EnsureType(local.LocalType);
            }

            // Handle method body
            var il = methodBody.GetILAsByteArray();
            if (il == null) return;
                
            foreach (var type in IlDecompiler.Decompile(method, il).Select(oper => oper.Operand).OfType<MemberInfo>().SelectMany(HandleMember))
            {
                EnsureType(type);
            }
        }

        private static IEnumerable<Type> HandleMember(MemberInfo info)
        {
            // Event, Field, Method, Constructor or Property.
            yield return info.DeclaringType;
            var eventInfo = info as EventInfo;
            if (eventInfo != null)
            {
                yield return eventInfo.EventHandlerType;
            }
            else
            {
                var fieldInfo = info as FieldInfo;
                if (fieldInfo != null)
                {
                    yield return fieldInfo.FieldType;
                }
                else
                {
                    var propertyInfo = info as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        yield return propertyInfo.PropertyType;
                    }
                    else
                    {
                        var constructorInfo = info as ConstructorInfo;
                        if (constructorInfo != null)
                        {
                            foreach (var par in constructorInfo.GetParameters())
                            {
                                yield return par.ParameterType;
                            }
                        }
                        else
                        {
                            var methodInfo = info as MethodInfo;
                            if (methodInfo != null)
                            {
                                foreach (var par in methodInfo.GetParameters())
                                {
                                    yield return par.ParameterType;
                                }
                            }
                            else
                            {
                                var type = info as Type;
                                if (type != null)
                                {
                                    yield return type;
                                }
                                else
                                {
                                    throw new NotSupportedException("Incorrect unsupported member type: " + info.GetType().Name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}