#ifndef _UTIL_HPP
#define _UTIL_HPP

// {0B20806A-328A-4e1c-931C-0250308574C7}
DEFINE_GUID(SAMPLE_NOTIFICATION_UI,    0xb20806a, 0x328a, 0x4e1c, 0x93, 0x1c, 0x2, 0x50, 0x30, 0x85, 0x74, 0xc7);

// {F6853F92-EB31-4e23-B6E7-FD69056153F0}  The MS Async UI type
DEFINE_GUID(MS_ASYNCNOTIFY_UI, 0xf6853f92, 0xeb31, 0x4e23, 0xb6, 0xe7, 0xfd, 0x69, 0x5, 0x61, 0x53, 0xf0);

typedef enum 
{
    SERVER_START_DOC        = 0x1,
    SERVER_END_DOC          = 0x2, 
    SERVER_END_DIALOG       = 0x3, 
    CLIENT_ACKNOWLEDGED     = 0xf
    
} EOEMDataSchema;

#endif
