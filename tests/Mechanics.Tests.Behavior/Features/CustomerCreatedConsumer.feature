Feature: Criação automática de usuário via evento de cliente

Scenario: Criar usuário quando cliente é criado
    Given que não existe usuário com o CPF "987.654.321-00"
    When o evento CustomerCreated é recebido com CPF "987.654.321-00" e customerId "37b52003-08d5-48d3-81d9-a9ee26c0bdad"
    Then um usuário é criado com role "Customer"
    And o customerId fica associado ao usuário
    And um e-mail de criação de senha é disparado

Scenario: Ignorar evento duplicado de cliente já existente
    Given que já existe usuário com o CPF "987.654.321-00"
    When o evento CustomerCreated é recebido novamente com o mesmo CPF
    Then nenhum usuário adicional é criado
    And nenhum e-mail é disparado
