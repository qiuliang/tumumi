using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;

namespace TMM.Core.Controller
{
    [Layout("default"),Rescue("error")]
    public class BaseController : SmartDispatcherController
    {
        public BaseController() {
            Service.Bll.Sys.S_ConfigBll configBll = new TMM.Service.Bll.Sys.S_ConfigBll();
                
            PropertyBag["sysConfig"] = configBll.Get();
        }


        public bool IsLogin
        {
            get {
                if (Session["logonUser"] != null) {
                    return true;
                }
                return false;
            }
            set {
                IsLogin = value;
            }
        }

        protected void AddError(string error, bool clearAll)
        {
            InternalGetValidationAware(clearAll).AddError(error);
        }

        protected void AddMessage(string message, bool clearAll)
        {
            InternalGetValidationAware(clearAll).AddMessage(message);
        }

        protected void AddSuccess(string success, bool clearAll)
        {
            InternalGetValidationAware(clearAll).AddSuccess(success);
        }

        protected void AddError(string error)
        {
            AddError(error, false);
        }

        protected void AddMessage(string message)
        {
            AddMessage(message, false);
        }

        protected void AddSuccess(string success)
        {
            AddSuccess(success, false);
        }

        protected void ClearValidation()
        {
            InternalGetValidationAware(true);
        }

        private ValidationAware InternalGetValidationAware(bool clearAll)
        {
            ValidationAware validationAware = Flash["validationAware"] as ValidationAware;
            if (validationAware == null)
            {
                validationAware = new ValidationAware();
                Flash["validationAware"] = validationAware;
            }
            else
            {
                if (clearAll)
                {
                    validationAware.Clear();
                }
            }
            return validationAware;
        }

        protected Model.U_UserInfo GetUser() {
            Model.U_UserInfo u = Session["logonUser"] as Model.U_UserInfo;
            return u;
        }
    }
}
