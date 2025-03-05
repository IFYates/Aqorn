# Aqorn
> **Acorn /ˈeɪkɔːn/:** The fruit of the oak tree and favoured food of squirrels.

Generate complex SQL inserts from a human-readable data model.

# Example
```json
// spec.jsonc
{
    "dbo.MyTable": {
        "Id": "!number",
        "Value": "?string"
    }
}
```
```json
// data.jsonc
{
    "dbo.MyTable": [
        { "Id": 1, "Value": "One" },
        { "Id": 2, "Value": "Two" }
    ]
}
```

```sql
-- MSSQL output
INSERT INTO [dbo].[MyTable]([Id], [Value]) VALUES
    (1, 'One'),
    (2, 'Two')
```

# Diagram
```mermaid
flowchart LR
    F1>spec file] -->|Parse| M1
    M1 --> E1[[Spec Errors]]
    D --> E4[[Write Errors]]
    D[Dataset] --> P
    M1[Spec Models] -->|Convert| D
    P[/SQL Writer/] --> S[[SQL]]
    O>Options] --> P
    M2 --> E2[[Data Errors]]
    M3 --> E3[[Data Errors]]
    M2[Data Models] -->|Add| D
    M3[Data Models] -->|Add| D
    F2>data file] -->|Parse| M2
    F3>data file] -->|Parse| M3
```