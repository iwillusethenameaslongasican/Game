using UnityEngine;
using System.Collections;
using code.unity.TankGame.Assets.proto.protoId;

public class ProtoHelper
{
    public static string IdToName(int typeId) {
        if (typeId == (int)ProtoId.c2s_login_request)
        {
            return "c2s_login_request";
        }
        else if (typeId == (int)ProtoId.s2c_login_reply)
        {
            return "s2c_login_reply";
        }
        else if (typeId == (int)ProtoId.c2s_register_request)
        {
            return "c2s_register_request";
        }
        else if (typeId == (int)ProtoId.s2c_register_reply)
        {
            return "s2c_register_reply";
        }
        else if (typeId == (int)ProtoId.c2s_logout_request)
        {
            return "c2s_logout_request";
        }
        else if (typeId == (int)ProtoId.s2c_logout_reply)
        {
            return "s2c_logout_reply";
        }
        else if (typeId == (int)ProtoId.c2s_heartbeat_request)
        {
            return "c2s_heartbeat_request";
        }
        else if (typeId == (int)ProtoId.s2c_heartbeat_reply)
        {
            return "s2c_heartbeat_reply";
        }
        else if (typeId == (int)ProtoId.c2s_get_achieve_request)
        {
            return "c2s_get_achieve_request";
        }
        else if (typeId == (int)ProtoId.s2c_get_achieve_reply)
        {
            return "s2c_get_achieve_reply";
        }
        else if (typeId == (int)ProtoId.c2s_get_room_list_request)
        {
            return "c2s_get_room_list_request";
        }
        else if (typeId == (int)ProtoId.s2c_get_room_list_reply)
        {
            return "s2c_get_room_list_reply";
        }
        else if (typeId == (int)ProtoId.c2s_create_room_request)
        {
            return "c2s_create_room_request";
        }
        else if (typeId == (int)ProtoId.s2c_create_room_reply)
        {
            return "s2c_create_room_reply";
        }
        else if (typeId == (int)ProtoId.c2s_enter_room_request)
        {
            return "c2s_enter_room_request";
        }
        else if (typeId == (int)ProtoId.s2c_enter_room_reply)
        {
            return "s2c_enter_room_reply";
        }
        else if (typeId == (int)ProtoId.c2s_get_room_info_request)
        {
            return "c2s_get_room_info_request";
        }
        else if (typeId == (int)ProtoId.s2c_get_room_info_reply)
        {
            return "s2c_get_room_info_reply";
        }
        else if (typeId == (int)ProtoId.c2s_leave_room_request)
        {
            return "c2s_leave_room_request";
        }
        else if (typeId == (int)ProtoId.s2c_leave_room_reply)
        {
            return "s2c_leave_room_reply";
        }
        else if (typeId == (int)ProtoId.c2s_start_fight_request)
        {
            return "c2s_start_fight_request";
        }
        else if (typeId == (int)ProtoId.s2c_start_fight_reply)
        {
            return "s2c_start_fight_reply";
        }
        else if (typeId == (int)ProtoId.c2s_fight_request)
        {
            return "c2s_fight_request";
        }
        else if (typeId == (int)ProtoId.s2c_fight_reply)
        {
            return "s2c_fight_reply";
        }
        else if (typeId == (int)ProtoId.c2s_update_unit_request)
        {
            return "c2s_update_unit_request";
        }
        else if (typeId == (int)ProtoId.s2c_update_unit_reply)
        {
            return "s2c_update_unit_reply";
        }
        else if (typeId == (int)ProtoId.c2s_shooting_request)
        {
            return "c2s_shooting_request";
        }
        else if (typeId == (int)ProtoId.s2c_shooting_reply)
        {
            return "s2c_shooting_reply";
        }
        else if (typeId == (int)ProtoId.c2s_hit_request)
        {
            return "c2s_hit_request";
        }
        else if (typeId == (int)ProtoId.s2c_hit_reply)
        {
            return "s2c_hit_reply";
        }
        else if (typeId == (int)ProtoId.c2s_result_request)
        {
            return "c2s_result_request";
        }
        else if (typeId == (int)ProtoId.s2c_result_reply)
        {
            return "s2c_result_reply";
        }
        else if (typeId == (int)ProtoId.c2s_chat_request)
        {
            return "c2s_chat_request";
        }
        else if (typeId == (int)ProtoId.s2c_chat_reply)
        {
            return "s2c_chat_reply";
        }
        else if (typeId == (int)ProtoId.c2s_group_chat_request)
        {
            return "c2s_group_chat_request";
        }
        else if (typeId == (int)ProtoId.s2c_group_chat_reply)
        {
            return "s2c_group_chat_reply";
        }
        else if (typeId == (int)ProtoId.c2s_friend_list_request)
        {
            return "c2s_friend_list_request";
        }
        else if (typeId == (int)ProtoId.s2c_friend_list_reply)
        {
            return "s2c_friend_list_reply";
        }
        else if (typeId == (int)ProtoId.c2s_add_friend_request)
        {
            return "c2s_add_friend_request";
        }
        else if (typeId == (int)ProtoId.s2c_add_friend_reply)
        {
            return "s2c_add_friend_reply";
        }
        else if (typeId == (int)ProtoId.c2s_del_friend_request)
		{
			return "c2s_del_friend_request";
		}
        else if (typeId == (int)ProtoId.s2c_del_friend_reply)
		{
			return "s2c_del_friend_reply";
		}
        else if (typeId == (int)ProtoId.c2s_friend_info_request)
		{
			return "c2s_friend_info_request";
		}
        else if (typeId == (int)ProtoId.s2c_friend_Info_reply)
		{
			return "s2c_friend_Info_reply";
		}
		return "";
    }

    public static int NameToId(string protoName) {
        if(protoName == "c2s_login_request") {
            return 10001;
        }
        else if(protoName == "s2c_login_reply") {
            return 10002;
        }
		else if (protoName == "c2s_register_request")
		{
			return 10003;
		}
		else if (protoName == "s2c_register_reply")
		{
			return 10004;
		}
		else if (protoName == "c2s_logout_request")
		{
			return 10005;
		}
		else if (protoName == "s2c_logout_reply")
		{
			return 10006;
		}
		else if (protoName == "c2s_heartbeat_request")
		{
			return 10007;
		}
		else if (protoName == "s2c_heartbeat_reply")
		{
			return 10008;
		}
		else if (protoName == "c2s_get_achieve_request")
		{
			return 20003;
		}
		else if (protoName == "s2c_get_achieve_reply")
		{
			return 20004;
		}
		else if (protoName == "c2s_get_room_list_request")
		{
			return 20005;
		}
		else if (protoName == "s2c_get_room_list_reply")
		{
			return 20006;
		}
		else if (protoName == "c2s_create_room_request")
		{
			return 20007;
		}
		else if (protoName == "s2c_create_room_reply")
		{
			return 20008;
		}
		else if (protoName == "c2s_enter_room_request")
		{
			return 20009;
		}
		else if (protoName == "s2c_enter_room_reply")
		{
			return 20010;
		}
		else if (protoName == "c2s_get_room_info_request")
		{
			return 20011;
		}
		else if (protoName == "s2c_get_room_info_reply")
		{
			return 20012;
		}
		else if (protoName == "c2s_leave_room_request")
		{
			return 20013;
		}
		else if (protoName == "s2c_leave_room_reply")
		{
			return 20014;
		}
		else if (protoName == "c2s_start_fight_request")
		{
			return 30001;
		}
		else if (protoName == "s2c_start_fight_reply")
		{
			return 30002;
		}
		else if (protoName == "c2s_fight_request")
		{
            return 30003;
		}
		else if (protoName == "s2c_fight_reply")
		{
			return 30004;
		}
		else if (protoName == "c2s_update_unit_request")
		{
			return 30005;
		}
		else if (protoName == "s2c_update_unit_reply")
		{
			return 30006;
		}
		else if (protoName == "c2s_shooting_request")
		{
			return 30007;
		}
		else if (protoName == "s2c_shooting_reply")
		{
			return 30008;
		}
		else if (protoName == "c2s_hit_request")
		{
			return 30009;
		}
		else if (protoName == "s2c_hit_reply")
		{
			return 30010;
		}
		else if (protoName == "c2s_result_request")
		{
			return 30011;
		}
		else if (protoName == "s2c_result_reply")
		{
			return 30012;
		}
		else if (protoName == "c2s_chat_request")
		{
			return 40001;
		}
		else if (protoName == "s2c_chat_reply")
		{
			return 40002;
		}
		else if (protoName == "c2s_group_chat_request")
		{
			return 40003;
		}
		else if (protoName == "s2c_group_chat_reply")
		{
			return 40004;
		}
		else if (protoName == "c2s_friend_list_request")
		{
			return 50001;
		}
		else if (protoName == "s2c_friend_list_reply")
		{
			return 50002;
		}
		else if (protoName == "c2s_add_friend_request")
		{
			return 50003;
		}
		else if (protoName == "s2c_add_friend_reply")
		{
			return 50004;
		}
		else if (protoName == "c2s_del_friend_request")
		{
			return 50005;
		}
		else if (protoName == "s2c_del_friend_reply")
		{
			return 50006;
		}
		else if (protoName == "c2s_friend_info_request")
		{
			return 50007;
		}
		else if (protoName == "s2c_friend_info_reply")
		{
			return 50008;
		}
        return 0;
    }
}
