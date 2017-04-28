using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;

namespace Windows.TaskSchedule.Utility
{
    public class Sandbox : MarshalByRefObject
    {
        private static AppDomain _domain = null;

        public static Sandbox Create(string domainName = null)
        {
            //方法一： 权限集和配置从创建的AppDomain继承
            domainName = domainName ?? "Sandbox" + new Random().Next(1000);
            _domain = AppDomain.CreateDomain(domainName, null, null);
            Sandbox box = _domain.CreateInstanceAndUnwrap(Assembly.GetAssembly(typeof(Sandbox)).FullName, typeof(Sandbox).ToString()) as Sandbox;
            return box;
        }

        public void Execute(string assemblyName, string typeName, string methodName, params object[] parameters)
        {
            var assembly = Assembly.Load(assemblyName);

            Type type = assembly.GetType(typeName);
            if (type == null)
                return;

            var instance = Activator.CreateInstance(type);

            MethodInfo method = type.GetMethod(methodName);
            method.Invoke(instance, parameters);
        }

        public void Dispose()
        {
            if (_domain != null)
            {
                AppDomain.Unload(_domain);

                _domain = null;
            }
        }

        //自定义对象租用周期
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override Object InitializeLifetimeService()
        {
            //将对象的租用周期改变为无限
            return null;         
        }
    }
}
