# Aqorn
> **Acorn /ˈeɪkɔːn/:** The fruit of the oak tree and favoured food of squirrels.

Generate complex SQL inserts from a human-readable data model.

# Usage
The command takes 3 input parameters:
1. [ `-s` / `--spec` ]: (Required) The location of the data model specification file.
2. [ `-d` / `--data` ]: (Required) One or more data files to parse against the specification.
3. `-p` / `--param`: Parameter overrides.
   - One or more named parameters in format `-p:name value`.
   - One or more files containing a dictionary of parameter values.
   - One or more dictionaries of parameter values.

Single file parsing:  
```sh
aqorn "spec.jsonc" "data.jsonc"
aqorn -s "spec.jsonc" -d "data.jsonc"
```

Multi-file parsing:  
```sh
aqorn "spec.jsonc" "data1.jsonc" "data2.jsonc" "data3.jsonc"
aqorn -s "spec.jsonc" -d "data1.jsonc" "data2.jsonc" "data3.jsonc"
```

Providing parameters:
```sh
aqorn "spec.jsonc" "data.jsonc" -p:name "Test" -p:state "Active"
aqorn -s "spec.jsonc" -d "data.jsonc" -p:name "Test" -p:state "Active"
```

# Schema

## Primitives
Possible **type** values are:
* `bool` / `boolean`
* `number`
* `string`
* `/{regular expression}/`

All must be prefixed with `!` for mandatory or `?` for optional.

A **complex value** is represented as an array, where each component can be a literal or a **reference**:
* `<{Field}`: Reference to field in same row
* `^{Field}`: Reference to field in parent row
* `@{Parameter}`: Reference to parameter at this scope
* `${SQL}`: Raw SQL statement

This makes string concatenation possible using `[ "string", " ", "string" ]`

A **subquery** is an object containing element `?`, with the following structure:
```jsonc
{
    "?": "[Schema.]Table.Field", // Full path of the field to retrieve
    "Field": "literal or complex value", // 0 or more fields to match
}
```

## Specification file
```jsonc
{
    "@Parameter": "type", // 0 or more global parameters
    "TableDefinition": { // Target table name or alias
        "#": "string", // Optional target table name
        ":identity": "boolean", // Optional boolean for whether this is an identity insert
        "@Parameter": "type", // 0 or more table parameters
        "Field": "type, literal, complex value, or subquery", // 1 or more table fields
        ":relations": { // Optional dictionary of inserts related to each table record
            // 1 or more TableDefinitions
        }
    }
}
```

## Data file
```jsonc
{
    "@Parameter": "literal, complex value, or subquery", // Values for global parameters
    "TableAlias": // Match for TableDefinition item
    [ // Array of records, or object if only need one
        {
            "@Parameter": "literal, complex value, or subquery", // Value for parameter at this scope
            "Field": "literal, complex value, or subquery", // Value for table field
            ":{TableAlias}": [] // Provide data for a related record (array or object)
        }
    ]
}
```

# Example
```jsonc
// spec.jsonc
{
    "dbo.MyTable": { // Table to insert into
        "Id": "!number", // Required numeric value
        "Value": "?string" // Optional string value
    }
}
```
```jsonc
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

# Future
* `":children": [ Table ]` to insert child data (inc `#` for table target)
* Repeat for parameter set (i.e., provide file containing array of parameter dictionaries and result is full set)
* Table aliases to be unique across whole spec
* Relationship can use existing table by alias  
    `{ TableA: { Relations: { TableB: { overrides } }, TableB: {} }`
* Maths (+, -, *, /, %, ^)  
    `[ "=", number, op, number, ... ]`
* String manipulation (length, substring, upper, lower)
    `[ "$op", value ] ]` -> `[ [ "sub", "value", 0, [ "=", [ "len", "value" ], "-", 2 ] ]`
* Binary values `0x...`  
    inc. concat
* Date values  
    inc. adjust, diff
* Reference multi-depth parent  
    `"^^Field"`
* Reference parent insert
    e.g., "^ParentId" for inserted identity
* Reference other table value  
    e.g, `{ "@": "TableAlias.Field", "Field1": "Value1", "??": "Fallback" }`
* Options
    * Output as JSONC valid against spec
    * Use transaction
    * Exclude print progress statements
    * Size of insert batch
    * Insert structure choices?
* Yaml source support
* Reference child insert
    e.g., Parent.:relations.Child insert id is used by Parent (">Child.ChildId")
* Limit relationships to 1 item