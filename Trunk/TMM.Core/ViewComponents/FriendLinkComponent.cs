using System;
using Castle.MonoRail.Framework;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using System.Collections;



namespace TMM.Core.ViewComponents
{
    /// <summary>
    /// ”—«È¡¥Ω”
    /// </summary>
    public class FriendLinkComponent : ViewComponent
    {
              

        public override void Render()
        {
            Service.SystemService sysService = RailsContext.GetService<Service.SystemService>();
            PropertyBag["friendLinks"] = sysService.SFriendLinkBll.GetList();
            base.Render();
        }
    }
}
