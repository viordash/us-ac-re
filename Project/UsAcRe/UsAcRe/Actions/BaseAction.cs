﻿
using System;
using System.ComponentModel;
using UsAcRe.Exceptions;

namespace UsAcRe.Actions {

	public abstract class BaseAction {
		protected NLog.Logger logger = NLog.LogManager.GetLogger("UsAcRe.FormMain");

		public abstract string ExecuteAsScriptSource();
		public abstract void Execute();

		protected void SafeAction(Action action) {
			try {
				action();
			} catch(Exception ex) {
				if(ex is Win32Exception && (uint)((Win32Exception)ex).ErrorCode == 0x80004005) {
					throw new MinorException(this);
				} else {
					throw new SevereException(this);
				}
			}

		}
	}
}
