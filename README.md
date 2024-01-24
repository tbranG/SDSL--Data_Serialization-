# SDSL (Data Serialization)
Custom language for data serialization

SDSL stands for *Simple Data Serialization Language*, the objective it's to be simpler than JSON, and more organized than YAML

## Sintax
Attributes are defined by using the dollar mark character `$`
```
$value 12
$alpha 0.7
$color red
```
The attribute value comes right after (separated by an whitespace). Strings doesn't need quotation marks

### Nested types

#### Objects
An attribute value can be another object. To define an object as the attribute value, encapsulate it with parentheses `()`
```
$background (
  $offsetX 0
  $offsetY -100
)
```

#### Arrays
An attribute composed of multiple values is called an array. To declare an array you need to open brackets `[]`, and after every element (the last one doesn't need it), insert an comma `,` 
```
$axis [
  Up,
  Down,
  Left,
  Right
]
```
Simple arrays can be written in a single line like `$volunteers [ Peter, Nora, Megan ]` <br><br>
Array holding objects doesn't need parentheses. Insert an comma to separete the objects
```
$chocolateBox [
  $chocolateName Mundy
  $chocolateType white,
  $chocolateName Eclipse
  $chocolateType dark
]
```

## Current Limitations (or currently not supported)
List of future features:
- ~~Negative numbers and float values~~
- Date types
- Arrays of objects
- Nested Arrays
