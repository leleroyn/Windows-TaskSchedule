
# Windows-TaskSchedule
Windows下的任务调试框架， 支持Cron表达式，支持任务以插件形式添加，支持部署为windows服务...
使用注意事项：

1. 要发布成WindowsService,直接在命令行执行 Windows.TaskSchedule.exe install,卸载用Windows-TaskSchedule.exe uninstall,具体参考topshelf组件的用法。
2. 添加新任务可能将任务插件（dll文件）直接放在根目录下，并配置好configs下的Jobs.config文件。
3. 所有任务都需要实现Windows.TaskSchedule.JobFactory.Ijob接口

2015-8-28

新增任务类型，支持执行exe等可执行程序。

2017-1-1

新增沙盒模式

2018-1-8
1.dll反射模式强制使用沙盒模式，增强宿主主程序的稳定性.
2.dll反射模式必须在参数中指定工作目录：workDir，默认为Bin目录.
3.dll反射模式默认会加载workDir中与dll同名的后缀为.config的配置文件.
4.Windows.TaskSchedule.JobFactory.Ijob 接口只保留Excute方法.
5.此版本不再兼容2.0版本。
