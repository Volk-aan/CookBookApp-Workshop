using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace CookBook.Helper
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
                App.Current.MainPage.DisplayAlert("!! Exception...", exception.ToString(), "Ok");
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
                if (await App.Current.MainPage.DisplayAlert("Fonctionnalité(s) indisponible(s)", "Souhaitez-vous accéder aux paramètres et donner les autorisations requises à l'application?", "Oui", "Non"))
                    CrossPermissions.Current.OpenAppSettings();

                return false;
            }

            return true;
        }
    }
}
