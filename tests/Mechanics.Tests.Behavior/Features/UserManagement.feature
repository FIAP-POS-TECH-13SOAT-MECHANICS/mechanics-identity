Feature: Gerenciamento de usuários

Scenario: Criar usuário com sucesso
    Given que não existe usuário com o CPF "436.549.470-85"
    And existe uma role com o nome "ATTENDANT" e id "a1097867-aa3e-416c-8685-190516b62a12"
    When eu envio uma requisição de criação com nome "João Silva", CPF "436.549.470-85", email "joao.silva@email.com" e role "a1097867-aa3e-416c-8685-190516b62a12"
    Then o usuário é criado

Scenario: Impedir criação de usuário com CPF duplicado
    Given que já existe usuário com o CPF "123.456.789-09"
    When eu envio uma requisição de criação com o mesmo CPF
    Then a resposta é 400 BadRequest

Scenario: Alterar senha do usuário
    Given que existe um usuário com id "4c3b8777-6c4a-4bf3-8ad5-aad48981f7f2"
    When eu altero a senha para "NovaSenha@2025"
    Then a senha é atualizada com hash correto
    And um evento UserChanged é publicado na fila
