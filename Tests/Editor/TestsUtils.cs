using System.Collections;
using System.Threading.Tasks;

namespace SaveSystem.Tests.Editor
{
    public static class TestsUtils
    {
        public static IEnumerator RunTaskAsCoroutine(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}