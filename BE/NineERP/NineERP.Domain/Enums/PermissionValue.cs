namespace NineERP.Domain.Enums
{
    public class PermissionValue
    {
        public const string PermissionType = "permission";

        public class Dashboard
        {
            public const string View = "DashboardView";
        }

        public class Report
        {
            public const string View = "ReportView";
            public const string Update = "ReportUpdate";
            public const string Add = "ReportAdd";
            public const string Delete = "ReportDelete";
        }
        public class System
        {
            public const string View = "SystemView";
        }

        public class Users
        {
            public const string View = "UserView";
            public const string Update = "UserUpdate";
            public const string Add = "UserAdd";
            public const string Delete = "UserDelete";
        }

        public class Roles
        {
            public const string View = "RolesView";
            public const string Update = "RolesUpdate";
            public const string Add = "RolesAdd";
            public const string Delete = "RolesDelete";
        }
        public class AuditLogs
        {
            public const string View = "AuditLogsView";
            public const string Export = "AuditLogsExport";
        }
        public class GeneralSettings
        {
            public const string View = "GeneralSettingsView";
            public const string Update = "GeneralSettingsUpdate";
        }
        public class EmailSettings
        {
            public const string View = "EmailSettingsView";
            public const string Update = "EmailSettingsUpdate";
        }
        public class EmailTemplates
        {
            public const string View = "EmailTemplatesView";
            public const string Update = "EmailTemplatesUpdate";
            public const string Add = "EmailTemplatesAdd";
            public const string Delete = "EmailTemplatesDelete";
        }
    }
}
