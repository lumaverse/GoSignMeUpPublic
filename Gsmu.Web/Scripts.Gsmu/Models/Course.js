﻿Ext.define('Course', {
    extend: 'Ext.data.Model',
    idProperty: 'COURSEID',
    fields: [
        { name: 'COURSEID', type: 'int' },
        { name: 'COURSENUM', type: 'string' },
        { name: 'COURSENAME', type: 'string' },
        { name: 'DISTPRICE', type: 'float' },
        { name: 'NODISTPRICE', type: 'float' },
        { name: 'LOCATION', type: 'string' },
        { name: 'LOCATIONURL', type: 'string' },
        { name: 'STREET', type: 'string' },
        { name: 'CITY', type: 'string' },
        { name: 'STATE', type: 'string' },
        { name: 'ZIP', type: 'string' },
        { name: 'ROOM', type: 'string' },
        { name: 'TIMES', type: 'date' },
        { name: 'MAXENROLL', type: 'int' },
        { name: 'MAXWAIT', type: 'int' },
        { name: 'INSTRUCTORID', type: 'int' },
        { name: 'INSTRUCTORID2', type: 'int' },
        { name: 'INSTRUCTORID3', type: 'int' },
        { name: 'DESCRIPTION', type: 'string' },
        { name: 'MAINCATEGORY', type: 'string' },
        { name: 'MAINCATEGORY2', type: 'string' },
        { name: 'MAINCATEGORY3', type: 'string' },
        { name: 'SUBCATEGORY1', type: 'string' },
        { name: 'SUBCATEGORY2', type: 'string' },
        { name: 'SUBCATEGORY1b', type: 'string' },
        { name: 'SUBCATEGORY2b', type: 'string' },
        { name: 'SUBCATEGORY1c', type: 'string' },
        { name: 'SUBCATEGORY2c', type: 'string' },
        { name: 'MATERIALS', type: 'string' },
        { name: 'DAYS', type: 'int' },
        { name: 'CREDITHOURS', type: 'float' },
        { name: 'SubStatus', type: 'int' },
        { name: 'NUMSUBSAVAIL', type: 'string' },
        { name: 'EVENTNUM', type: 'string' },
        { name: 'ACCOUNTNUM', type: 'string' },
        { name: 'CANCELCOURSE', type: 'int' },
        { name: 'EmailReminderType', type: 'int' },
        { name: 'EmailReminderSubject', type: 'string' },
        { name: 'EmailReminderBody', type: 'string' },
        { name: 'Notes', type: 'string' },
        { name: 'InternalClass', type: 'int' },
        { name: 'PriceTypes', type: 'string' },
        { name: 'nonPriceTypes', type: 'string' },
        { name: 'DisplayPrice', type: 'int' },
        { name: 'MaterialsRequired', type: 'int' },
        { name: 'Audience', type: 'int' },
        { name: 'AudienceID', type: 'int' },
        { name: 'GradingSystem', type: 'int' },
        { name: 'BuybackCourse', type: 'int' },
        { name: 'DepartmentNameID', type: 'int' },
        { name: 'StartEndTimeDisplay', type: 'string' },
        { name: 'InserviceHours', type: 'float' },
        { name: 'Icons', type: 'string' },
        { name: 'LinkTitle', type: 'string' },
        { name: 'LinkURL', type: 'string' },
        { name: 'ExcludedOptions', type: 'string' },
        { name: 'ContactName', type: 'string' },
        { name: 'ContactPhone', type: 'string' },
        { name: 'CourseColorGrouping', type: 'int' },
        { name: 'Special1', type: 'string' },
        { name: 'CourseCommentText', type: 'string' },
        { name: 'courseoutlineid', type: 'int' },
        { name: 'ShowTopCatalog', type: 'int' },
        { name: 'ShowBoldCatalog', type: 'int' },
        { name: 'AllowCreditRollover', type: 'int' },
        { name: 'CustomCreditHours', type: 'float' },
        { name: 'CEUCredit', type: 'float' },
        { name: 'GraduateCredit', type: 'float' },
        { name: 'Requirements', type: 'int' },
        { name: 'OnlineCourse', type: 'int' },
        { name: 'CustomCourseField1', type: 'string' },
        { name: 'CustomCourseField2', type: 'string' },
        { name: 'HalfDayCourse', type: 'int' },
        { name: 'SpecialDistPrice1', type: 'float' },
        { name: 'Duplicated', type: 'int' },
        { name: 'WaitListJobDate', type: 'date' },
        { name: 'SubSiteId', type: 'int' },
        { name: 'ExternalKey', type: 'string' },
        { name: 'SubSiteCourseImportKey', type: 'string' },
        { name: 'CourseConfirmationEmailExtraText', type: 'string' },
        { name: 'ShowCreditRequirementLink', type: 'int' },
        { name: 'CourseCloseDays', type: 'int' },
        { name: 'LastUpdateTime', type: 'date' },
        { name: 'Percent4MultiEnrollDiscount', type: 'int' },
        { name: 'MinStudentNum4MultiEnrollDiscount', type: 'int' },
        { name: 'CustomCourseField3', type: 'string' },
        { name: 'CustomCourseField4', type: 'string' },
        { name: 'LocationAdditionalInfo', type: 'string' },
        { name: 'RoomDirectionsId', type: 'int' },
        { name: 'CoursesType', type: 'int' },
        { name: 'CollectExtraParticipant', type: 'int' },
        { name: 'courseinternalaccesscode', type: 'string' },
        { name: 'CourseStudentInfoRequired', type: 'string' },
        { name: 'OfflineStudentNames', type: 'string' },
        { name: 'Membership', type: 'int' },
        { name: 'OfflineStudentCount', type: 'int' },
        { name: 'LDAPGroupMatch', type: 'string' },
        { name: 'ElectronicSignatureFile', type: 'string' },
        { name: 'ElectronicSignatureFile2', type: 'string' },
        { name: 'ElectronicSignatureFile3', type: 'string' },
        { name: 'CourseTypeAltVerbiage', type: 'string' },
        { name: 'BBCourseCloned', type: 'int' },
        { name: 'CoursesHideFromCatalog', type: 'int' },
        { name: 'Optionalcredithours1', type: 'float' },
        { name: 'ShowPrerequisite', type: 'int' },
        { name: 'PrerequisiteInfo', type: 'string' },
        { name: 'RemindInstructor', type: 'int' },
        { name: 'bb_last_integration_state', type: 'string' },
        { name: 'bb_last_integration_date', type: 'date' },
        { name: 'bb_last_update_grade', type: 'date' },
        { name: 'coursecertificate', type: 'int' },
        { name: 'CourseSpecificEnrollmentCheck', type: 'int' },
        { name: 'viewpastcoursesdays', type: 'int' },
        { name: 'ShortDescription', type: 'string' },
        { name: 'SpecialCourseType', type: 'int' },
        { name: 'CustomCourseField5', type: 'string' },
        { name: 'CustomCourseField6', type: 'string' },
        { name: 'mandatory', type: 'int' },
        { name: 'freeclass', type: 'int' },
        { name: 'NoRegEmail', type: 'int' },
        { name: 'useRoommateMatch', type: 'int' },
        { name: 'coursetype', type: 'int' },
        { name: 'eventid', type: 'int' },
        { name: 'sessionid', type: 'int' },
        { name: 'BBServer', type: 'int' },
        { name: 'bbautoenroll', type: 'int' },
        { name: 'forimportonly', type: 'int' },
        { name: 'Country', type: 'string' },
        { name: 'PartialPaymentAmount', type: 'string' },
        { name: 'PartialPaymentSP', type: 'string' },
        { name: 'PartialPaymentNon', type: 'string' },
        { name: 'heliuslms_last_integration', type: 'date' },
        { name: 'heliuslmscloned', type: 'int' },
        { name: 'archived_course', type: 'int' },
        { name: 'google_calendar', type: 'string' },
        { name: 'google_calendar_import_enable', type: 'int' },
        { name: 'StudentChoiceCourse', type: 'int' },
        { name: 'CourseCertificationsId', type: 'int' },
        { name: 'SendConfirmationEmailtoInstructor', type: 'int' },
        { name: 'attendanceThreshold', type: 'string' },
        { name: 'MinMultiEnrolDiscountAll', type: 'int' },
        { name: 'Amount4MultiEnrollDiscount', type: 'string' },
        { name: 'SingleCreditCost', type: 'float' },
        { name: 'Optionalcredithours2', type: 'float' },
        { name: 'Optionalcredithours3', type: 'float' },
        { name: 'Optionalcredithours4', type: 'float' },
        { name: 'Optionalcredithours5', type: 'float' },
        { name: 'Optionalcredithours6', type: 'float' },
        { name: 'Optionalcredithours7', type: 'float' },
        { name: 'Optionalcredithours8', type: 'float' },
        { name: 'CustomDropDownValueSequence', type: 'string' },
        { name: 'showcreditinpublic', type: 'int' },
        { name: 'AllowSendSurvey', type: 'int' },
        { name: 'TileImageUrl', type: 'string' },
        { name: 'haiku_course_id', type: 'int' },
        { name: 'haiku_import_id', type: 'string' },
        { name: 'haiku_integration_date', type: 'date' },
        { name: 'haiku_last_integration_date', type: 'date' },
        { name: 'haiku_last_result', type: 'string' },
        { name: 'disablehaikuintegration', type: 'int' },
        { name: 'disable_canvas_integration', type: 'int' },
        { name: 'canvas_course_id', type: 'int' }
    ]
});


