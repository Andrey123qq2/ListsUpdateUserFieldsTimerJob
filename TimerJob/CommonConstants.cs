namespace ListsUpdateUserFieldsTimerJob
{
    class CommonConstants
    {
        internal static readonly string LIST_PROPERTY_JSON_CONF = "tj_updlistusrattr_json_conf";
        internal static readonly string LIST_PROPERTY_PERM_JSON_CONF = "er_perm_json_conf";
        internal static readonly string TJOB_PROPERTY_JSON_CONF = "tj_updlistusrattr_common_json_conf";
        internal static readonly string TIMER_JOB_NAME = "Lists Update User Fields TimerJob";
        internal static readonly string TJOB_SITE_FEATURE_NAME = "ListsUpdateUserFieldsTimerJob_Feature2";
        internal static readonly int CHANGE_MANAGER_DAYS_TO_CHECK = 2;
        internal static readonly string ERROR_MESSAGE_TEMPLATE = "Custom TimerJob Exception: {0}, {1} " + "[ {2} ].";
    }
}