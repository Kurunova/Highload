box.cfg{
    listen = 3301
}

box.once("schema", function()
    local dialogs = box.schema.space.create('dialogs')
    dialogs:format({
        {name = 'id', type = 'unsigned'},
        {name = 'dialog_id', type = 'string'},
        {name = 'from_user_id', type = 'unsigned'},
        {name = 'to_user_id', type = 'unsigned'},
        {name = 'text', type = 'string'},
        {name = 'sent_at', type = 'string'} 
    })
end)

-- Создаем составной первичный ключ (dialog_id, id)
box.space.dialog_messages:create_index('primary', {
    parts = {
        {field = 'dialog_id', type = 'string'},
        {field = 'id', type = 'unsigned'}
    },
    if_not_exists = true
})

-- Дополнительные индексы
box.space.dialog_messages:create_index('from_user_id_index', {
    parts = {'from_user_id'},
    unique = false,
    if_not_exists = true
})
box.space.dialog_messages:create_index('to_user_id_index', {
    parts = {'to_user_id'},
    unique = false,
    if_not_exists = true
})