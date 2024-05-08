global using TKCliTool;
using System.Reflection;
using TKCliTool.Tools;

var argCount = args.Length;

//run different code based on the number of arguments
if(argCount == 0){
    new UsageTool().Run(args);
    return;
}

var subcommand = args[0];
//get class with ToolAttribute which name is equal to subcommand
var type = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<ToolAttribute>()?.Name == subcommand)
    .FirstOrDefault();
if(type == null){
    new UsageTool().Run(args);
    return;
}else{
    var tool = (ToolBase)Activator.CreateInstance(type);
    if(tool == null){
        new UsageTool().Run(args);
        return;
    }
    tool.Run(args.Skip(1).ToArray());
}