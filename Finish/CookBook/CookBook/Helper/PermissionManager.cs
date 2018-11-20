using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace CookBook.Helpers
{
    public static class PermissionsManager
    {
        private static async Task<Permission[]> AskPermissions(Permission[] permissions)
        {
            List<Permission> neededPermissions = new List<Permission>();
            List<Permission> refusedPermissions = new List<Permission>();

            try
            {
                foreach (Permission permission in permissions)
                {
                    var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

                    if (permissionStatus != PermissionStatus.Granted)
                        neededPermissions.Add(permission);
                }

                if (neededPermissions.Count == 0) // all permissions already granted
                    return refusedPermissions.ToArray();

                var permissionsDictionary = await CrossPermissions.Current.RequestPermissionsAsync(neededPermissions.ToArray());
                foreach (var permissionKeyPair in permissionsDictionary)
                {
                    if (permissionKeyPair.Value != PermissionStatus.Granted)
                        refusedPermissions.Add(permissionKeyPair.Key);
                }
            }
            catch (Exception exception)
            {
                //Helper.DisplayAlert("!! Exception...", exception.ToString());
            }

            return refusedPermissions.ToArray();
        }

        /// <summary>
        /// Requests all given permissions if not already granted
        /// </summary>
        /// <param name="permissions"> Array of permissions to request</param>
        /// <returns>If one or more permissions are refused then returns false</returns>
        public static async Task<bool> RequestPermissions(Permission[] permissions)
        {
            Permission[] refusedPermissions = await AskPermissions(permissions);
            if (refusedPermissions.Length > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Feature(s) unavailable", "Would you like to access the settings and give the required permissions to the app?", "Ok");
                CrossPermissions.Current.OpenAppSettings();
            }

            return true;
        }
    }
}