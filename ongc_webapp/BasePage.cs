using System;
using System.Web.UI;

namespace ongc_webapp
{
    /// <summary>
    /// Central security gatekeeper. All protected pages must inherit from this class.
    /// It runs BEFORE any page lifecycle event, ensuring no content is ever rendered
    /// to an unauthorized user.
    /// </summary>
    public abstract class BasePage : Page
    {
        // Override in any child page to enforce a specific role.
        // Example: protected override string RequiredRole => "admin";
        protected virtual string RequiredRole => null;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            EnforceAccess();
        }

        private void EnforceAccess()
        {
            // ── STEP 1: AUTHENTICATION ─────────────────────────────────────────
            // Both session keys must exist. If either is missing, the session has
            // expired or the user was never authenticated. Send them to login.
            if (Session["UserRole"] == null || Session["UserCPF"] == null)
            {
                Response.Redirect("~/Login.aspx", true);
                return; // Unreachable after Redirect(endResponse:true), but good practice
            }

            // ── STEP 2: AUTHORIZATION ──────────────────────────────────────────
            // Only check role if this specific page declares a RequiredRole.
            // Pages without RequiredRole are accessible to any authenticated user.
            if (!string.IsNullOrEmpty(RequiredRole))
            {
                string userRole = Session["UserRole"]?.ToString();

                // OrdinalIgnoreCase handles DB inconsistencies: 'Admin', 'admin', 'ADMIN'
                if (!string.Equals(userRole, RequiredRole, StringComparison.OrdinalIgnoreCase))
                {
                    // Role mismatch — user is logged in but not authorized for this page.
                    // Redirect to the general employee landing page.
                    Response.Redirect("~/Indexing.aspx", true);
                    return;
                }
            }
        }
    }
}