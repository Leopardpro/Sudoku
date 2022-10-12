# 代码分析器

## 介绍

这个项目甚至提供了对代码进行分析的项目。

| 分析编号    | 类型                     | 级别 | 信息内容                                                     |
| ----------- | ------------------------ | ---- | ------------------------------------------------------------ |
| SCA0001[^1] | 代码分析（CodeAnalysis） | ❌    | 特殊类型缺失                                                 |
| SCA0002     | 代码生成（CodeGen）      | ❌    | 代码分析器缺失特性标记                                       |
| SCA0003     | 代码生成（CodeGen）      | ⚠    | `RegisteredPropertyNamesAttribute` 的第二个参数必须至少有一个值 |
| SCA0004     | 代码生成（CodeGen）      | ⚠    | 代码分析器类型应用 `sealed` 修饰，并且不能用 `abstract` 修饰 |
| SCA0005     | 代码生成（CodeGen）      | ❌    | `RegisterOperationActionAttribute` 不支持该类型内容的分析    |
| SCA0006     | 代码生成（CodeGen）      | ⚠    | 代码分析器名称命名不规范                                     |
| SCA0101     | 使用（Usage）            | ⚠    | 请勿对大结构类型进行无参实例化                               |
| SCA0102     | 使用（Usage）            | ⚠    | 大结构类型的参数需要按引用传递                               |
| SCA0201     | 使用（Usage）            | ℹ    | 请使用 `Argument` 类型代替 `if` 判断                         |
| SCA0202     | 性能（Performance）      | ℹ    | 请使用 `Add` 方法代替 `operator +` 运算符                    |
| SCA0203     | 性能（Performance）      | ℹ    | 请使用 `Remove` 方法代替 `operator -` 运算符                 |

## 注解

[^1]: 所有以“SCA00”起头的代码分析规则都是源代码生成器内部提供的分析。由于 C# 自身的限制，所有这些规则的级别只可能是警告（⚠图标）或错误（❌图标）这两种情况。