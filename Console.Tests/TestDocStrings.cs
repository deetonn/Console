using Console.Commands.Builtins;
using Console.Commands.Builtins.Config;
using Console.Commands.Builtins.DirBased;
using Console.Commands.Builtins.Etc;
using Console.Commands.Builtins.Informational;
using Console.Commands.Builtins.System;
using Console.Commands.Builtins.Web;
using System.Windows.Input;

namespace Console.Tests;

/*
 * This set of tests check that the `docs` command works as expected for all commands.
 * It also verifies that all commands have implemented a docstring.
 * It also makes sure that any doc strings are not using invalid syntax for markup.
 * If there is any invalid syntax, an exception will be thrown and the test will fail.
 * this lets us catch these things early.
*/

[TestClass]
public class TestDocStrings
{
    private IConsole _instance = DummyConsole.Get;

    private void RunTestsFor(Commands.ICommand command)
    {
        Assert.AreNotEqual(string.Empty, command.DocString);
        Assert.AreEqual(0, _instance.Commands.ExecuteFrom(_instance, new ViewDocCommand().Name, command.Name));
    }

    // Builtins/Config

    [TestMethod]
    public void TestEditOptionsCommand()
    {
        RunTestsFor(new EditOptionCommand());
    }

    [TestMethod]
    public void TestListPluginsCommand()
    {
        RunTestsFor(new ListPluginsCommand());
    }

    [TestMethod]
    public void TestLoadPluginsCommand()
    {
        RunTestsFor(new LoadPluginCommand());
    }

    [TestMethod]
    public void TestReloadConfigCommand()
    {
        RunTestsFor(new ReloadConfigCommand());
    }

    [TestMethod]
    public void TestRemoveOptionCommand()
    {
        RunTestsFor(new RemoveOptionCommand());
    }

    [TestMethod]
    public void TestUnloadPluginCommand()
    {
        RunTestsFor(new UnloadPluginCommand());
    }

    [TestMethod]
    public void TestViewOptionsCommand()
    {
        RunTestsFor(new ViewOptionsCommand());
    }

    // Builtins/DirBased

    [TestMethod]
    public void TestCdCommand()
    {
        RunTestsFor(new ChangeDirectoryCommand());
    }

    [TestMethod]
    public void TestCopyCommand()
    {
        RunTestsFor(new CopyCommand());
    }

    [TestMethod]
    public void TestDirCommand()
    {
        RunTestsFor(new DirCommand());
    }

    [TestMethod]
    public void TestLineCountCommand()
    {
        RunTestsFor(new LineCountCommand());
    }

    [TestMethod]
    public void TestRmDirCommand()
    {
        RunTestsFor(new RmDirCommand());
    }

    [TestMethod]
    public void TestTouchCommand()
    {
        RunTestsFor(new TouchCommand());
    }

    // Builtins/Etc

    [TestMethod]
    public void TestAliasCommand()
    {
        RunTestsFor(new AliasCommand());
    }

    [TestMethod]
    public void TestClearCommand()
    {
        RunTestsFor(new ClearBufferCommand());
    }

    [TestMethod]
    public void TestGenerateCommand()
    {
        RunTestsFor(new GenerateCommand());
    }

    [TestMethod]
    public void TestQueueCommand()
    {
        RunTestsFor(new QueueCommand());
    }

    [TestMethod]
    public void TestVwfCommand()
    {
        RunTestsFor(new ViewFileCommand());
    }

    [TestMethod]
    public void TestViewManyFilesCommand()
    {
        RunTestsFor(new ViewManyFilesCommand());
    }

    // Builtins/Informational

    [TestMethod]
    public void TestAboutCommand() // useless as fuck
    {
        RunTestsFor(new AboutCommand());
    }

    [TestMethod]
    public void TestHelpCommand()
    {
        RunTestsFor(new HelpCommand());
    }

    [TestMethod]
    public void TestLastRanAtCommand()
    {
        RunTestsFor(new LastRanAtCommand());
    }

    [TestMethod]
    public void TestDocCommand()
    {
        RunTestsFor(new ViewDocCommand());
    }

    // Builtins/System

    [TestMethod]
    public void TestEchoCommand()
    {
        RunTestsFor(new EchoCommand());
    }

    // SKIPPED: ExitCommand -- it is internal

    [TestMethod]
    public void TestRunCommand()
    {
        RunTestsFor(new RunCommand());
    }

    [TestMethod]
    public void TestTaskListCommand()
    {
        RunTestsFor(new TaskListCommand());
    }

    // SKIPPED: tray commands -- they are shit

    // Builtins/Web
    [TestMethod]
    public void TestPkgInstallCommand()
    {
        RunTestsFor(new PkgInstall());
    }

    [TestMethod]
    public void TestsPkgListCommand()
    {
        RunTestsFor(new PkgList());
    }
}