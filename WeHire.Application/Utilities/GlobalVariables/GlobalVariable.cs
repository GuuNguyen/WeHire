using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.GlobalVariables
{
    public static class GlobalVariable
    {
        public static class NotificationTypeString
        {
            public const string HIRING_REQUEST = "Hiring Request";
            public const string INTERVIEW = "Interview";
            public const string AGREEMENT = "Agreement";
            public const string TASK = "Task";
            public const string PAYMENT = "Payment";
        }

        public static class Reason
        {
            public const string REJECTED_REASON = "Your hiring request have been rejected";
        }

        public static class ResponseMessage
        {
            public const string CREATE_SUCCESS = "Created successfully!";
            public const string CREATE_SELECTED_DEV_SUCCESS = "Select developer to request successfully!";
        }


        public static class ChildFolderName
        {
            public const string CV_FOLDER = "cv";
            public const string AVATAR_FOLDER = "avatar";
            public const string LOGO_FOLDER = "logo";
            public const string BACKGROUND_FOLDER = "background";
        }

        public static class ErrorMessage
        {        
            public const string NULL_FIELD = "This field is null!";
            public const string REFRESH_TOKEN_FIELD = "The refresh token is invalid or expired!";
            public const string USER_NOT_EXIST = "This user does not exist!";
            public const string USER_DEVICE_NOT_EXIST = "This user device does not exist!";
            public const string HR_NOT_EXIST = "This HR does not exist!";
            public const string STAFF_NOT_EXIST = "This staff does not exist!";
            public const string CURRENT_PASSWORD_NOT_MATCH = "Current password is incorrect";
            public const string PROJECT_NOT_MATCH = "projectId not match";
            public const string PROJECT_NOT_EXIST = "This project not exist!";
            public const string PROJECT_TYPE_NOT_MATCH = "project type not match";
            public const string PROJECT_TYPE_NOT_EXIST = "project type does not exist!";
            public const string NEW_PASSWORD_NOT_MATCH = "New password does not match";
            public const string DEV_NOT_EXIST = "This developer does not exist!";
            public const string LIST_DEV_NULL = "List developer is null!";
            public const string DEV_COUNT = "One or more developers have entered invalid";
            public const string LEVEL_NOT_EXIST = "This level does not exist!";
            public const string SKILL_NOT_EXIST = "This skill does not exist!";
            public const string AGREEMENT_NOT_EXIST = "This agreement does not exist!";
            public const string TASK_NOT_EXIST = "This task does not exist!";
            public const string DEV_TASK_NOT_EXIST = "This DeveloperTaskAssignment does not exist!";
            public const string HIRING_REQUEST_NOT_EXIST = "This hiring request does not exist!";
            public const string SELECTING_DEV_NOT_EXIST = "This selecting dev does not exist!";
            public const string INTERVIEW_NOT_EXIST = "This interview does not exist!";
            public const string COMPANY_NOT_EXIST = "This company does not exist!";
            public const string TYPE_NOT_EXIST = "This type of developer does not exist!";
            public const string STATUS_NOT_EXIST = "This status does not exist!";
            public const string CV_NOT_EXIST = "This CV does not exist!";
            public const string INCORRECT_INFO = "Please enter correct information!";
            public const string NOT_ALLOWS = "You are not allowed to use this function!";
            public const string NO_LONGER_ACTIVE = "This user is no longer active!";
            public const string EMAIL_ALREADY_EXIST = "This email already exists!";
            public const string NULL_REQUEST_BODY = "Request body is null!";
            public const string PHONE_NUMBER_ALREADY_EXIST = "Phone number already exists!";
        }
      
        public static class ErrorField
        {
            public const string ID_FIELD  = "Id";
            public const string REFRESH_TOKEN  = "RefreshToken";
            public const string USER_DEVICE  = "UserDevice";
            public const string PROJECT_FIELD  = "Project";
            public const string USER_FIELD = "User";
            public const string STAFF_FIELD = "Staff";
            public const string HR_FIELD = "HR";
            public const string PASSWORD_FIELD = "Password";
            public const string HIRING_REQUEST_FIELD = "hiringRequest";
            public const string AGREEMENT_FIELD = "agreementId";
            public const string TASK_FIELD = "Task";
            public const string DEV_TASK_FIELD = "DeveloperTaskAssignment";
            public const string SELECTING_DEV_FIELD = "selectedDev";
            public const string COMPANY_FIELD = "companyId";
            public const string DEV_FIELD = "developerId";
            public const string INTERVIEW_FIELD = "Interview";
            public const string PROJECT_TYPE_FIELD = "ProjectType";
            public const string REQUEST_BODY = "Request body";
            public const string USER_ID_FIELD = "User Id";
            public const string STATUS_FIELD = "Status";
            public const string EMAIL_FIELD = "Email";
            public const string ROLE_FIELD = "Role";
            public const string LEVEL_FIELD = "Level";
            public const string SKILL_FIELD = "Skill";
            public const string TYPE_FIELD = "Type";
            public const string CV_FIELD = "CV";
            public const string PHONE_NUMBER_FIELD = "PhoneNumber";
        }
    }
}
