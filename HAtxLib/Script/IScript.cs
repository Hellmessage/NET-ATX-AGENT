using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Script {
	public interface IScript {

		void UpdateProgress(string text);

        bool RunScript(HAtx atx);

	}
}