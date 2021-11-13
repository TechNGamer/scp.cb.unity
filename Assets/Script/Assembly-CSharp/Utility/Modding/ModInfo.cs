namespace SCPCB.Remaster.Utility.Modding {
	public struct ModInfo {

		public string Name {
			get;
		}

		public string RootPath {
			get;
		}

		internal ModInfo( string name, string rootPath ) {
			Name     = name;
			RootPath = rootPath;
		}

	}
}
