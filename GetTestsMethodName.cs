using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bulk
{
    public class Bulk
    {
        public void GetAllTestsName()
        {
            Assembly ass = Assembly.Load(AssemblyName.GetAssemblyName("pass to **.dll"));
			// filter for classes
            List<Type> types = ass.GetTypes().Where(t => t.FullName.StartsWith("Tests.Jobseeker") && t.Name.EndsWith("Tests")).ToList();

            Dictionary<string, Dictionary<string, List<string>>> tests = new Dictionary<string, Dictionary<string, List<string>>>();

            foreach (Type t in types)
            {
                List<string> methodNames = new List<string>();

                var folderName = t.FullName.Remove(t.FullName.LastIndexOf('.'));
                var className = t.FullName.Split('.')[3];

                Dictionary<string, List<string>> classMethodName = new Dictionary<string, List<string>>();

                foreach (MemberInfo method in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => m.CustomAttributes.Any(a => a.ConstructorArguments.Any(n => n.Value.ToString().EndsWith("Smoke") || n.Value.ToString().EndsWith("Regression")))))
                {
                    methodNames.Add(method.Name);
                }

                if (classMethodName.ContainsKey(className))
                {
                    classMethodName[className].AddRange(methodNames);
                }
                else
                {
                    classMethodName.Add(className, methodNames);
                }
                if (tests.ContainsKey(folderName))
                {
                    var previosNames = tests[folderName];
                    foreach(var cmn in classMethodName)
                    {
                        previosNames.Add(cmn.Key, cmn.Value);
                    }
                }
                else
                {
                    tests.Add(folderName, classMethodName);
                }
            }
            foreach (var f in tests)
            {
                foreach(var c in f.Value)
                {
                    int i = 1;
                    foreach (var m in c.Value)
                    {
                        Console.WriteLine("{0,-3} {1,-30}{2,-40}{3,0}", i + ".", f.Key, c.Key, m);
                        i++;
                    }
                }
            }
        }
    }
}
