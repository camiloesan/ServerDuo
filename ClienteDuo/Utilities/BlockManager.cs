using ClienteDuo.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteDuo.Utilities
{
    public static class BlockManager
    {
        public static int BlockUserByUsername(string blockerUsername, string blockedUsername)
        {
            BlockManagerClient blockManagerClient = new BlockManagerClient();
            return blockManagerClient.BlockUserByUsername(blockerUsername, blockedUsername);
        }

        public static int UnblockUserByBlockId(int blockId)
        {
            BlockManagerClient blockManagerClient = new BlockManagerClient();
            return blockManagerClient.UnblockUserByBlockId(blockId);
        }

        public static bool IsUserBlocked(string usernameBlocker, string usernameBlocked)
        {
            BlockManagerClient blockManagerClient = new BlockManagerClient();
            return blockManagerClient.IsUserBlockedByUsername(usernameBlocker, usernameBlocked);
        }

        public static IEnumerable<UserBlockedDTO> GetBlockedUsersListByUserId(int userId)
        {
            BlockManagerClient blockManagerClient = new BlockManagerClient();
            return blockManagerClient.GetBlockedUsersListByUserId(userId);
        }
    }
}
