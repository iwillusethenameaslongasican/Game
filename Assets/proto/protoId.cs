//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: code/unity/TankGame/Assets/proto/protoId.proto
namespace code.unity.TankGame.Assets.proto.protoId
{
    [global::ProtoBuf.ProtoContract(Name=@"ProtoId")]
    public enum ProtoId
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_login_request", Value=10001)]
      c2s_login_request = 10001,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_login_reply", Value=10002)]
      s2c_login_reply = 10002,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_register_request", Value=10003)]
      c2s_register_request = 10003,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_register_reply", Value=10004)]
      s2c_register_reply = 10004,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_logout_request", Value=10005)]
      c2s_logout_request = 10005,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_logout_reply", Value=10006)]
      s2c_logout_reply = 10006,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_heartbeat_request", Value=10007)]
      c2s_heartbeat_request = 10007,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_heartbeat_reply", Value=10008)]
      s2c_heartbeat_reply = 10008,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_get_achieve_request", Value=20003)]
      c2s_get_achieve_request = 20003,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_get_achieve_reply", Value=20004)]
      s2c_get_achieve_reply = 20004,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_get_room_list_request", Value=20005)]
      c2s_get_room_list_request = 20005,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_get_room_list_reply", Value=20006)]
      s2c_get_room_list_reply = 20006,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_create_room_request", Value=20007)]
      c2s_create_room_request = 20007,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_create_room_reply", Value=20008)]
      s2c_create_room_reply = 20008,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_enter_room_request", Value=20009)]
      c2s_enter_room_request = 20009,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_enter_room_reply", Value=20010)]
      s2c_enter_room_reply = 20010,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_get_room_info_request", Value=20011)]
      c2s_get_room_info_request = 20011,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_get_room_info_reply", Value=20012)]
      s2c_get_room_info_reply = 20012,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_leave_room_request", Value=20013)]
      c2s_leave_room_request = 20013,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_leave_room_reply", Value=20014)]
      s2c_leave_room_reply = 20014,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_start_fight_request", Value=30001)]
      c2s_start_fight_request = 30001,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_start_fight_reply", Value=30002)]
      s2c_start_fight_reply = 30002,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_fight_request", Value=30003)]
      c2s_fight_request = 30003,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_fight_reply", Value=30004)]
      s2c_fight_reply = 30004,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_update_unit_request", Value=30005)]
      c2s_update_unit_request = 30005,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_update_unit_reply", Value=30006)]
      s2c_update_unit_reply = 30006,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_shooting_request", Value=30007)]
      c2s_shooting_request = 30007,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_shooting_reply", Value=30008)]
      s2c_shooting_reply = 30008,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_hit_request", Value=30009)]
      c2s_hit_request = 30009,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_hit_reply", Value=30010)]
      s2c_hit_reply = 30010,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_result_request", Value=30011)]
      c2s_result_request = 30011,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_result_reply", Value=30012)]
      s2c_result_reply = 30012,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_chat_request", Value=40001)]
      c2s_chat_request = 40001,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_chat_reply", Value=40002)]
      s2c_chat_reply = 40002,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_group_chat_request", Value=40003)]
      c2s_group_chat_request = 40003,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_group_chat_reply", Value=40004)]
      s2c_group_chat_reply = 40004,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_friend_list_request", Value=50001)]
      c2s_friend_list_request = 50001,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_friend_list_reply", Value=50002)]
      s2c_friend_list_reply = 50002,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_add_friend_request", Value=50003)]
      c2s_add_friend_request = 50003,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_add_friend_reply", Value=50004)]
      s2c_add_friend_reply = 50004,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_del_friend_request", Value=50005)]
      c2s_del_friend_request = 50005,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_del_friend_reply", Value=50006)]
      s2c_del_friend_reply = 50006,
            
      [global::ProtoBuf.ProtoEnum(Name=@"c2s_friend_info_request", Value=50007)]
      c2s_friend_info_request = 50007,
            
      [global::ProtoBuf.ProtoEnum(Name=@"s2c_friend_Info_reply", Value=50008)]
      s2c_friend_Info_reply = 50008
    }
  
}