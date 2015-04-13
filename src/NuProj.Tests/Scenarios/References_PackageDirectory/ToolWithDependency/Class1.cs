using System.Collections.Immutable;

namespace ToolWithDependency
{
    public class Class1
    {
        public void Foo()
        {
            var immutableList = ImmutableList.Create<int>();
        }
    }
}
