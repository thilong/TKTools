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
