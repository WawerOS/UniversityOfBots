[Back to overview](../README.md)

---

## Send message commands:

- [send channel](#send-channel)
- [send dm](#send-dm)
- [send disableDMs](#send-disableDMs)

---

### send channel

**Description**: Anonymously sends a message to a specified channel

**Parameters**:

| name        | format | default | description                                              |
| ----------- | ------ | ------- | -------------------------------------------------------- |
| channelName | text   | -       | Full or partial name of the channel you want to post in. |
| message     | text   | -       | Your message text.                                       |

If you don't want to type out the whole channel name, you can use just the beginning of the name, as long as it's unambigious.
For example: `!g send shouting I AM ANGRY` will send the message to `shouting-only`.

**Aliases**: _none_

**Short form:** `send`

**Examples**:
`!g send channel the-lobby Hello!`
`!g send the-lobby Hello!`

### send dm

**Description**: Anonymously sends a message to a user via DM.

**Note**: This command might be disabled or restricted. Users can also disable the receiving of DMs through Gauss.

**Parameters**:

| name     | format | default | description                                       |
| -------- | ------ | ------- | ------------------------------------------------- |
| receiver | text   | -       | Name or @mention of the user you want to message. |
| message  | text   | -       | Your message text.                                |

**Aliases**: _none_

**Short form:** _none_

**Examples**:
`!g send dm MaxMustermann Hello!`

### send disableDMs

**Description**: Disable or enable receiving anonymous DMs through Gauss.

**Parameters**:

| name  | format            | default | description                                                                      |
| ----- | ----------------- | ------- | -------------------------------------------------------------------------------- |
| block | `true` or `false` | `true`  | True, if you want to enable the blocking. False if you want to disable the block |

**Aliases**: `blockDMs`

**Short form:** _none_

**Examples**:
`!g send blockDMs true` - Block anonymous DMs.
`!g send blockDMs false` - Allow anonymous DMs.
