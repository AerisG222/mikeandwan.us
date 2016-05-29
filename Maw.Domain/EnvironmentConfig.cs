using System;


namespace Maw.Domain
{
	public class EnvironmentConfig
	{
		public string Name { get; set; }
		public string AssetsPath { get; set; }
		public string DbConnectionString { get; set; }
        
        public bool IsProduction {
            get
            {
                return !IsDevelopment;
            }
        }
        
        public bool IsDevelopment {
            get
            {
                return string.Equals(Name, "Development", StringComparison.OrdinalIgnoreCase);
            }
        }
	}
}
