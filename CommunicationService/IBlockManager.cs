using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationService
{
    [ServiceContract]
    internal interface IBlockManager
    {
        /// <summary>
        /// Adds user to blocked user's list
        /// </summary>
        /// <param name="blockerUsername"></param>
        /// <param name="blockedUsername"></param>
        /// <returns>Rows affected: 1 if succeed, 2 if banned definitely</returns>
        [OperationContract]
        int BlockUserByUsername(string blockerUsername, string blockedUsername);

        /// <summary>
        /// Deletes user from user's block list
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns>Rows affected</returns>
        [OperationContract]
        int UnblockUserByBlockId(int blockId);

        /// <summary>
        /// Checks if user is blocked by another user
        /// </summary>
        /// <param name="usernameBlocker"></param>
        /// <param name="usernameBlocked"></param>
        /// <returns>True if is blocked, false if not</returns>
        [OperationContract]
        bool IsUserBlockedByUsername(string usernameBlocker, string usernameBlocked);

        /// <summary>
        /// Gets user's blocked list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List UserBlockedDTO</returns>
        [OperationContract]
        List<UserBlockedDTO> GetBlockedUsersListByUserId(int userId);
    }
}
