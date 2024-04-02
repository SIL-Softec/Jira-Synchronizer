using JiraSynchronizer.Application.ViewModels;
using JiraSynchronizer.Core.Enums;
using JiraSynchronizer.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Services;

public class AuthorizationService
{
    public bool IsAuthorized(List<UserViewModel> userList, List<ProjektMitarbeiterViewModel> projektMitarbeiterList, string email, int projectId)
    {
        LoggingService logService = new LoggingService();
        // Check if T_USER table contains a user with the given email adress
        if (!userList.Any(u => u.UniqueName == email))
        {
            logService.Log(LogCategory.UserNotFound, $"Benutzer mit Email {email} konnte nicht gefunden werden");
            return false;
        }

        // Check if TZ_PROJEKT_MITARBEITER contains a user with the id we got from T_USER and wether any of the entries with that id also contain the proper project id
        if (!projektMitarbeiterList.Any(pm => pm.MitarbeiterId == userList.First(u => u.UniqueName == email).MitarbeiterId && pm.ProjektId == projectId))
        {
            logService.Log(LogCategory.UserNotAuthorized, $"Benutzer mit Email {email} ist nicht authorisiert auf das Projekt mit Id {projectId} zu buchen");
            return false;
        }

        return true;
    }
}
