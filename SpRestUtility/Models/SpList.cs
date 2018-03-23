﻿using System.Collections.Generic;

namespace SpRestUtility
{
    public class SpList
    {
        public string Id { get; set; }
        public SpListTemplateType SpListTemplateType { get; set; }
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public void SetProperty(string propertyName, string value)
        {
            if (Properties.ContainsKey(propertyName))
                Properties[propertyName] = value;
            else
                Properties.Add(propertyName, value);
        }
    }
    public enum SpListTemplateType
    {
        InvalidType = -1,
        NoListTemplate = 0,
        GenericList = 100,
        DocumentLibrary = 101,
        Survey = 102,
        Links = 103,
        Announcements = 104,
        Contacts = 105,
        Events = 106,
        Tasks = 107,
        DiscussionBoard = 108,
        PictureLibrary = 109,
        DataSources = 110,
        WebTemplateCatalog = 111,
        UserInformation = 112,
        WebPartCatalog = 113,
        ListTemplateCatalog = 114,
        XMLForm = 115,
        MasterPageCatalog = 116,
        NoCodeWorkflows = 117,
        WorkflowProcess = 118,
        WebPageLibrary = 119,
        CustomGrid = 120,
        SolutionCatalog = 121,
        NoCodePublic = 122,
        ThemeCatalog = 123,
        DesignCatalog = 124,
        AppDataCatalog = 125,
        DataConnectionLibrary = 130,
        WorkflowHistory = 140,
        GanttTasks = 150,
        HelpLibrary = 151,
        AccessRequest = 160,
        TasksWithTimelineAndHierarchy = 171,
        MaintenanceLogs = 175,
        Meetings = 200,
        Agenda = 201,
        MeetingUser = 202,
        Decision = 204,
        MeetingObjective = 207,
        TextBox = 210,
        ThingsToBring = 211,
        HomePageLibrary = 212,
        Posts = 301,
        Comments = 302,
        Categories = 303,
        Facility = 402,
        Whereabouts = 403,
        CallTrack = 404,
        Circulation = 405,
        Timecard = 420,
        Holidays = 421,
        IMEDic = 499,
        ExternalList = 600,
        MySiteDocumentLibrary = 700,
        IssueTracking = 1100,
        AdminTasks = 1200,
        HealthRules = 1220,
        HealthReports = 1221,
        DeveloperSiteDraftApps = 1230
    }
}