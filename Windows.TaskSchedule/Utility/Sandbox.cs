using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace Windows.TaskSchedule.Utility
{
    public class Sandbox : MarshalByRefObject
    {
        private static AppDomain _domain = null;

        public static Sandbox Create(string config, string workDir)
        {
            string domainName = Guid.NewGuid().ToString();
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationName = domainName;
            //应用程序根目录
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            //子目录（相对形式）在AppDomainSetup中加入外部程序集的所在目录，多个目录用分号间隔
            ads.PrivateBinPath = workDir;
            ////设置缓存目录
            //ads.CachePath = ads.ApplicationBase;
            ////获取或设置指示影像复制是打开还是关闭
            //ads.ShadowCopyFiles = "true";
            ////获取或设置目录的名称，这些目录包含要影像复制的程序集
            //ads.ShadowCopyDirectories = ads.ApplicationBase;

            ads.DisallowBindingRedirects = false;
            ads.DisallowCodeDownload = true;
            if (!string.IsNullOrEmpty(config))
            {
                ads.ConfigurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, workDir, config);
            }

            //Create evidence for the new application domain from evidence of
            Evidence adevidence = AppDomain.CurrentDomain.Evidence;

            _domain = AppDomain.CreateDomain(domainName, adevidence, ads);
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
