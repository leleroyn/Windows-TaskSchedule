# Windows-TaskSchedule
Windows下的任务调试框架， 支持Cron表达式，支持任务以插件形式添加，支持部署为windows服务...
使用注意事项：
1.要发布成WindowsService,直接在命令行执行 Windows.TaskSchedule.exe install,卸载用Windows-TaskSchedule.exe uninstall,具体参考topshelf组件的用法。
2.添加新任务可能将任务插件（dll文件）直接放在根目录下，并配置好configs下的Jobs.config文件。
3.所有任务都需要实现Windows.TaskSchedule.JobFactory.Ijob接口

2015-8-28
新增任务类型，支持执行exe等可执行程序。
