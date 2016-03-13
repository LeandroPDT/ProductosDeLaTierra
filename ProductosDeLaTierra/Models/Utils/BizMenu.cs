using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

public class BizMenu {
    public string Texto;
    public string Link;
    public string ID;
    public string CssClass;
    public string Descrip;
    public List<BizMenu> subMenu;

    public int PermisoID;

    public BizMenu() {}

    public BizMenu(string Texto, string Link, string ID) {
        this.Texto = Texto;
        this.Link = Link;
        this.ID = ID;

    }
    public BizMenu(string Texto, string Link, string ID, string CssClass)
        : this(Texto, Link, ID) {
        this.CssClass = CssClass;
    }

    public BizMenu(string Texto, string Link, string ID, string CssClass, string Descrip)
        : this(Texto, Link, ID, CssClass) {
        this.Descrip = Descrip;
    }

    public BizMenu(string Texto, string Link, string ID, int PermisoID)
        : this(Texto, Link, ID) {
        this.PermisoID = PermisoID;
    }
    public BizMenu(string Texto, string Link, string ID, string CssClass, int PermisoID)
        : this(Texto, Link, ID, CssClass) {
        this.PermisoID = PermisoID;
    }

    public BizMenu(string Texto, string Link, string ID, string CssClass, string Descrip, int PermisoID)
        : this(Texto, Link, ID, CssClass, Descrip) {
        this.PermisoID = PermisoID;
    }


    public bool IsActive() {
        return System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower().EndsWith(this.Link.ToLower());
    }

    public bool IsActiveAsParent() {
        return System.Web.HttpContext.Current.Request.Url.PathAndQuery.ToLower().StartsWith(this.Link.ToLower());
    }

    public bool IsAnySubMenuActive() {
		if (this.subMenu == null) {
			return false;
		} else {
			return (from rec in this.subMenu where rec.IsActive() select rec).Any();
		}


	}


}

