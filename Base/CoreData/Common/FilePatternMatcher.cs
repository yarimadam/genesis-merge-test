using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace CoreData.Common
{
    // For further information about glob patterns see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-5.0
    public class FilePatternMatcher
    {
        private readonly string _basePath;
        private readonly Matcher _matcher = new Matcher();

        public FilePatternMatcher(string basePath, IReadOnlyCollection<string> excludePatternsGroups)
        {
            _basePath = basePath;

            _matcher.AddInclude("**/*");
            _matcher.AddExcludePatterns(excludePatternsGroups);
        }

        public bool IsAllowed(string filePath) => _matcher.Match(_basePath, filePath).HasMatches;

        public IEnumerable<string> GetAllowedFiles(IEnumerable<string> files)
        {
            files = files.Select(file => !Path.IsPathRooted(file) ? Path.Combine(_basePath, file) : file);

            return _matcher.Match(_basePath, files).Files.Select(file => file.Path);
        }

        public static FilePatternMatcher Load(string basePath, string fileContent)
        {
            var excludePatternsGroups = fileContent
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            return new FilePatternMatcher(basePath, excludePatternsGroups);
        }
    }
}