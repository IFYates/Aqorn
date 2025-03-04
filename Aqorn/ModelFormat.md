# Features
Use JSON/YAML to quickly represent a complex set of SQL inserts, including creating related records.

* Model validation
* Query other tables
* Concatenate values
* Dynamic properties
* Embedded SQL

# Spec
```json
{
    "{alias}":
    {
        "#": table_name?, // If omitted, uses object key name
        ":identity": bool?, // Does this definition explictly set the row identity
        ":relations": { // Optional object of related items
            relation
        },
        field_or_parameter*
    }
}
```

`table_name` = `{schema_name}.{table_name}` or `{table_name}`

`field_or_parameter` = `field` or `parameter`

`field` = `"{name}": field_spec`

`parameter` = `"@{name}": field_type`

`field_spec` = `optional_field` or `required_field`

`optional_field` = `"?{field_type}"`

`required_field` = `"?{field_type}"`

`field_type` = "number" or "string" or "string(len)" or `concatenation` or `subquery`

`concatenation` = `[`

## Examples
**Basic table insert**
```json
{
    "dbo.Table": {
        "Key": "!string(50)",
        "Value": "?string"
    }
}
```