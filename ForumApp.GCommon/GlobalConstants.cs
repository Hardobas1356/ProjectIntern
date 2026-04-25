namespace ForumApp.GCommon;

public static class GlobalConstants
{
    public static string APPLICATION_DATE_TIME_FORMAT = "yyyy-MM-dd";

    public static class DeletedUser
    {
        public const string DELETED_USERNAME = "Deleted user";
        public const string DELETED_DISPLAYNAME = "Deleted user";
        public const string DELETED_EMAIL = "Deleted user";
    }

    public static class Roles
    {
        public const string ADMIN_ROLE_NAME = "Admin";
        public const string USER_ROLE_NAME = "Intern";
    }

    public static class Pages
    {
        public static int USER_PAGE_SIZE = 10;
        public static int ADMIN_BOARD_PAGE_SIZE = 12;
        public static int BOARD_PAGE_SIZE = 10;
        public static int POST_PAGE_SIZE = 10;
        public static int REPLY_PAGE_SIZE = 10;
    }
}