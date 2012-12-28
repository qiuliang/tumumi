using System;

using Castle.MonoRail.Framework;

namespace TMM.Core.ViewComponents
{
    /// <summary>
    /// 出错或操作提示信息组件
    /// </summary>
    public class ValidationAwareComponent : ViewComponent
    {
        public override void Render()
        {
            ValidationAware validationAware = Flash["validationAware"] as ValidationAware;
            if (validationAware != null)
            {
                PropertyBag["validationAware"] = validationAware;
                Flash.Remove("validationAware");
            }
            base.Render();
        }
    }
}
