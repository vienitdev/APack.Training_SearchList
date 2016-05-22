namespace Archpack.Training.ArchUnits.Environment.V1
{
    public interface IApplicationEnvironment
    {
        string EnvironmentName { get; }

        string RootDir { get; }

        string MapPath(string virtualPath);

        string ApplicationRoot { get; }
    }
}