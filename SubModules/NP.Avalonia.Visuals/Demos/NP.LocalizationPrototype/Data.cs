using NP.Utilities;
using System.ComponentModel;

namespace NP.LocalizationPrototype
{
    public class Data : VMBase
	{
		private static int _sid = 0;

		private int _id = ++_sid;

		public int ID
		{
			get { return _id; }
		}


        #region Uid Property
        private string _uid;
        public string Uid
        {
            get
            {
                return this._uid;
            }
            set
            {
                if (this._uid == value)
                {
                    return;
                }

                this._uid = value;
                this.OnPropertyChanged(nameof(Uid));
            }
        }
        #endregion Uid Property

        public Data(string uid)
		{
			this._uid = uid;
		}
	}
}
