using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MonoRail.Framework;

namespace TMM.Core.Controller.Admin
{
    [Layout("default_admin")]
    [ControllerDetails(Area = "admin")]
    [Filter(ExecuteEnum.BeforeAction,typeof(Filter.AdminFilter))]
    public class BaseController : SmartDispatcherController
    {
        


        
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

        protected int ErrorCount
        {
            get
            {
                var valid = Flash["validationAware"] as ValidationAware;
                if (valid == null)
                    return 0;
                return valid.Errors.Count;
            }
        }

        protected Model.U_UserInfo GetUser() {
            Model.U_UserInfo u = Session["logonUser"] as Model.U_UserInfo;
            return u;
        }

        protected void SuccessInfo() {
            AddSuccess("操作成功");
        }

        protected void RedirectToReferrer2(string url)
        {
            if (string.IsNullOrEmpty(Context.UrlReferrer))
                Redirect(url);
            else
                base.RedirectToReferrer();
        }

        
    }
}
