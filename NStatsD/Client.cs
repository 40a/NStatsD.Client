using System;
using System.Collections.Generic;
using System.Configuration;

namespace NStatsD
{
    //public class BucketNameFragment
    //{
    //    private string[] _fragments;

    //    public BucketNameFragment(params string[] fragments)
    //    {
            
    //    }

    //    public override string ToString()
    //    {
    //        return String.Join(".", _fragments);
    //    }
    //}

    public static class NStatsDClient
    {
        private static readonly Lazy<StatsDConfigurationSection> _config = new Lazy<StatsDConfigurationSection>(() => (StatsDConfigurationSection)ConfigurationManager.GetSection("statsD"));

        /// <summary>
        /// So we don't have to repeat MachineName.App.Name.etc everywhere
        /// </summary>
        public static string GlobalBucketPrefix = string.Empty;

        public static StatsDConfigurationSection Config
        {
            get
            {
                return _config.Value;
            }
        }

        public static StatBucket With(string name)
        {
            return WithWorker(GlobalBucketPrefix, name);
        }

        public static StatBucket With(params string[] nameTree)
        {
            return WithWorker(GlobalBucketPrefix, nameTree);
        }

        public static StatBucket WithoutPrefix(params string[] nameTree)
        {
            return WithWorker(null, nameTree);
        }

        private static StatBucket WithWorker(string prefix, params string[] nameTree)
        {
            return new StatBucket(Config, GetBucketName(prefix, nameTree));
        }

        public static string GetBucketName(params string[] nameTree)
        {
            return String.Join(".", nameTree);
        }

        private static string GetBucketName(string prefix, params string[] nameTree)
        {
            var resolvedName = String.Join(".", nameTree);
            if (!string.IsNullOrEmpty(prefix))
            {
                resolvedName = prefix + "." + resolvedName;
            }
            return resolvedName;
        }
    }
}
