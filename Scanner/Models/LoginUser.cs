using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Scanner.Models
{
    public class LoginUser
    {
        //[Required(ErrorMessage = "* Login Email is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Login Email Input (Max 50 letters)")]
        [Display(Name = "Login Email")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "* Password is null")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Password Input (Max 20 letters)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Phone/Mobile Input (Max 50 letters)")]
        [Display(Name = "Phone/Mobile")]
        public string Phone { get; set; }
        public IList<string> PhoneList { get; set; }


        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }


        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public User IsValid(string _username, string _password)
        {
            IList<User> users = new List<User>();
            User user = new User();
            using (var context = new DbContext(Global.ConnStr))
            {


                object[] parameters = { _username, _password };
                try
                {
                    users = context.Database.SqlQuery<User>("proc_GetValidUser {0},{1}", parameters).ToList<User>();
                    user = (users.Count > 0) ? users[0] : null;

                }
                catch (ValidationException e)
                {

                    user = null;
                }
            }

            return user;
        }

        public User IsValid(string Phone, string Password, int f)
        {
            IList<User> users = new List<User>();
            User user = new User();
            using (var context = new DbContext(Global.ConnStr))
            {
                Password = (Password == null) ? "" : Password;
                object[] parameters = { Phone, Password };
                try
                {
                    context.Database.ExecuteSqlCommand("proc_GetValidUserByPhone_Step1 {0}, {1}", parameters);
                    users = context.Database.SqlQuery<User>("proc_GetValidUserByPhone_Step2 {0}, {1}", parameters).ToList<User>();
                    user = (users.Count > 0) ? users[0] : null;
                }
                catch (ValidationException e)
                {
                    user = null;
                }
            }
            return user;
        }
    }


    public class LoginUser2
    {

        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Login Email Input (Max 50 letters)")]
        [Display(Name = "Login Email")]
        public string UserName { get; set; }

        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Password Input (Max 20 letters)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }


        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public User IsValid(string _username, string _password)
        {
            IList<User> users = new List<User>();
            User user = new User();
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { _username, _password };
                try
                {
                    users = context.Database.SqlQuery<User>("proc_GetValidUser {0},{1}", parameters).ToList<User>();
                    user = (users.Count > 0) ? users[0] : null;

                }
                catch (ValidationException e)
                {

                    user = null;
                }
            }


            return user;
        }

    }


}