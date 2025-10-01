'Public Class ActionList
'   Inherits List(Of String)

'End Class

Public Class MethodList
   Inherits List(Of String)

End Class

'Public Class AuthenticatedTokenCollection
'   Inherits Dictionary(Of String, SysTokenInfo)  ' Token, TokenInfo

'End Class

Public Class TokenCollection
   Inherits Dictionary(Of String, Date)  ' Token, ExpiryDate

End Class

Public Class DbParameterList
   Inherits List(Of DbParameter)

End Class

Public Class KeyColumnList
   Inherits Dictionary(Of String, Object)

End Class

Public Class ModelList(Of T)
   Inherits List(Of T)

End Class

Public Class PageList
   Inherits List(Of PageInfo)

End Class

'Public Class SecuredActionsList
'   Inherits Dictionary(Of String, ActionList)

'End Class

Public Class SecuredMethodsList
   Inherits Dictionary(Of String, MethodList)

End Class
