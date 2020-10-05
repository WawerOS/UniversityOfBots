[Back to overview](../README.md)

---

## Administrative commands:

- [admin test](#admin-test)
- [admin addrole](#admin-addrole)
- [admin removerole](#admin-removerole)
- [admin command listdisabled](#admin-command-listdisabled)
- [admin command disable](#admin-command-disable)
- [admin command enable](#admin-command-enable)
- [admin command disableFor](#admin-command-disableFor)
- [admin command enableFor](#admin-command-enableFor)

---

### admin test

**Description**: Will answer you, if you have permission to use admin commands.

**Parameters**: _none_

**Aliases**: _none_

**Examples**: `!g admin test` - will answer "You have permissions to execute admin commands.", if you have permissions.

---

### admin addrole

**Description**: Give permissions to use any `admin` command to a server role.

Note: The Bot will prompt you to confirm the the action with a recap message and 2 reactions.
Click on the `✅` reaction to commit the change.

**Parameters**:

| name   | format | default | description                                                         |
| ------ | ------ | ------- | ------------------------------------------------------------------- |
| roleId | number | -       | Discord's ID of the role you want to give bot admin permissions to. |

Note 1: You can get the ID by activating Discords developer mode and right-clicking on a role in the server settings.

**Aliases**: _none_

**Examples**: `!g admin addrole 713490193407279155`

---

### admin removerole

**Description**: Remove permissions to use any `admin` command from a server role.

Note: The Bot will prompt you to confirm the the action with a recap message and 2 reactions.
Click on the `✅` reaction to commit the change.

**Parameters**:

| name   | format | default | description                                                      |
| ------ | ------ | ------- | ---------------------------------------------------------------- |
| roleId | number | -       | Discord's ID of the role want to strip of bot admin permissions. |

Note 1: You can get the ID by activating Discords developer mode and right-clicking on a role in the server settings.

**Aliases**: _none_

**Examples**: `!g admin addrole 713490193407279155`

---

### admin command listdisabled

**Description**: Lists commands that are currently disabled on the server.

**Parameters**: _none_

**Aliases**: _none_

**Examples**: `!g admin listdisabled`

---

### admin command disable

**Description**: Disable a command for all users on the server.

**Parameters**:

| name        | format | default | description               |
| ----------- | ------ | ------- | ------------------------- |
| commandName | text   | -       | Full name of the command. |

**Aliases**: _none_

**Examples**:

- `!g admin disable send` - Disable the entire `send` command group. Think: Every command that starts with `send `.
- `!g admin disable send dm` - Disable the `send dm` command specifically.

---

### admin command enable

**Description**: Reenable a command for all users on the server.

**Parameters**:

| name        | format | default | description               |
| ----------- | ------ | ------- | ------------------------- |
| commandName | text   | -       | Full name of the command. |

**Aliases**: _none_

**Examples**:

- `!g admin disable send` - Reenable the entire `send` command group. Think: Every command that starts with `send `.
- `!g admin disable send dm` - Reenable the `send dm` command specifically.

---

### admin command disableFor

**Description**:

**Parameters**:

| name | format | default | description |
| ---- | ------ | ------- | ----------- |
|      |        | -       |             |

**Aliases**:

**Examples**:

---

### admin command enableFor

**Description**:

**Parameters**:

| name | format | default | description |
| ---- | ------ | ------- | ----------- |
|      |        | -       |             |

**Aliases**:

**Examples**:

---
