using LibGit2Sharp;
using System.Diagnostics;

namespace CommonUtils
{
    public static class GitUtil
    {
        public static bool HasDriver = !ProcessUtil.Run("git").HasError;

        public static ProcessResult GetStatusInfo(string path)
        {
            if (!HasDriver || FileSystem.IsFile(path))
                return default;

            var markPath = path.Combine(".git");

            if (!FileSystem.IsFloder(markPath) && !FileSystem.IsFile(markPath))
                return default;

            //影响原有git的图标获取 好像与争抢仓库的IO有关
            var startInfo = new ProcessStartInfo("git");

            startInfo.Arguments = "status";
            startInfo.WorkingDirectory = path;

            return ProcessUtil.Start(startInfo, 1.5);
        }

        private const string _conflictKeywords = "(fix conflicts and run \"git commit\")";

        private const string _committedKeywords = "nothing to commit, working tree clean";

        private const string _notPushedKeywords = "(use \"git push\" to publish your local commits)";

        public static GitStatus GetStatus(string path)
        {
            var info = GetStatusInfo(path);

            if (info == default)
                return default;

            if (info.HasError)
                return GitStatus.Error;

            if (info.Result.Contains(_conflictKeywords))
                return GitStatus.Conflict;

            if (!info.Result.Contains(_committedKeywords))
                return GitStatus.Modified;

            if (info.Result.Contains(_notPushedKeywords))
                return GitStatus.Committed;

            return GitStatus.Pushed;
        }

        //todo dowith LibGit2Sharp
        public static GitStatus GetStatus1(string floder)
        {
            var repository = new Repository(floder);
            var info = repository.RetrieveStatus(new StatusOptions());

            return GitStatus.Committed;
        }

        public static bool CheckUpdate(string floder)
        {
            var repository = new Repository(floder);
            var info = repository.RetrieveStatus(new StatusOptions());
            return true;
        }
    }
}
