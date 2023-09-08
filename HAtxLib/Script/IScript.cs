using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAtxLib.Script {
	public interface IScript {

		void UpdateProgress(string text);
		void TaskSuccess();
		void TaskFailure(string text);

        void RunScript(HAtx atx);

	}
}