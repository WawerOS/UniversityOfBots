# Election audit log

This log contains an entry for every action that changes the state of the election poll, with a hash of the state both before the change and after the change (where applicable).

The idea is, that every change on the election data is traceable trough a the hash. If the data is tampered with or corrupted, a hash will deviate from what users get back from the bot and the results can be contested.

This system is not perfect, but it's a relatively easy solution to ensure that the results are authentic without logging each voters individual votes.

## Adding an election

```text
[2020-10-27 19:10] [Added election poll] Hash: 8fb8ab68f3371f67baaea25077a10f8e95514bbed47d80c778879abbcb67a34d
JSON:
<Redacted>
```

The JSON is omitted here, because it would contain usernames and user IDs. But you can verify the hash of the JSON with an online tool like this:

https://emn178.github.io/online-tools/sha256.html 

Simply copy the whole line of JSON into the tool and compare the resulting number.

## Post the message for the election

```text
[2020-10-27 19:11] [Post election poll message] Hash: 8fb8ab68f3371f67baaea25077a10f8e95514bbed47d80c778879abbcb67a34d
[2020-10-27 19:11] [Saved election poll message] Hash: 8409f8c536febc7491680c7e80d0e3a0b6c526c533a5be87462d35d1ddc68bf9
JSON:
<Redacted>
```

This step has it's own hash, because the Bot saves the message details on the election data. So, posting the message will change the hash.

## Adding votes

```text
[2020-10-27 19:11] [Add vote - before] Hash: 8409f8c536febc7491680c7e80d0e3a0b6c526c533a5be87462d35d1ddc68bf9
[2020-10-27 19:11] [Add vote - after] Hash: 38a19102bca60e234262d7448e19048682b4354eca7500dd01eb5b093c95c9a3
```

These must always match what a user has been given back by the bot.

## Closing the poll

```text
[2020-10-27 19:15] [Close election - before] Hash: 38a19102bca60e234262d7448e19048682b4354eca7500dd01eb5b093c95c9a3
[2020-10-27 19:15] [Close election- after] Hash: 655a93976da93394d5295693a35cd46d6b95e3cea231fe1bd2689cb44bb10790
JSON:
<Redacted>
```

Closing the poll flips a status indicator on the election, also resulting in a hash change. This step contains the full JSON representation again, so you can check the JSON again the results the bot posted and check the hash.
