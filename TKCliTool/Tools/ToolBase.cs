using System.Reflection;

namespace TKCliTool.Tools;

public abstract class ToolBase
{
    public abstract void printHelp();
    public abstract void Run(string[] args);

    public string AssemblyName => GetType().Assembly.GetName().Name;
}

public class ToolAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ToolAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public class ActionAttribute : Attribute
{
    public string Action { get; set; }
    public string ShortAction { get; set; }
    public string Description { get; set; }
    public string Arguments { get; set; }
    public ActionAttribute(string action, string shortAction, string description, string args = "")
    {
        Action = action;
        this.ShortAction = shortAction;
        Description = description;
        this.Arguments = args;
    }


}

public static class ToolsExtensions
{
    public static bool IsEqualToAction(this MethodInfo method, string action)
    {
        var attr = method.GetCustomAttribute<ActionAttribute>();
        if (attr == null)
            return false;
        return attr.Action == action || attr.ShortAction == action;
    }
}

