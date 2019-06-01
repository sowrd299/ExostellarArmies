reg_f = open('regular_ability_template.txt', 'r')
regular_template = reg_f.readlines()
reg_f.close()

pers_f = open('persistent_ability_template.txt', 'r')
persistent_template = pers_f.readlines()
pers_f.close()

temp_f = open('persistent_ability_template.txt', 'r')
temporary_template = temp_f.readlines()
temp_f.close()

file_dict = \
{\
'R': regular_template, \
'P': persistent_template, \
'T': temporary_template\
}

def case(real_name):
	return "".join(real_name.replace('.', '').replace('-', '').split(' '))

def file_name(card_name, ability_types, i):
	return f"{card_name}{('' if len(ability_types) == 1 else i+1)}"

def write_file(name, effect, ability_types, i):
	print(f'Trying to create "{name}" with effect "{effect}"')
	file_name = f"Assets/Scripts/GameSRC/Abilities/{name}.cs"
	try:
		f = open(file_name, 'r')
		f.close()
		if input('    File already exists, type "OVERRIDE" to delete it and write over it: ') == 'OVERRIDE':
			raise Error()
	except:
		with open(file_name, 'w') as wf:
			for line in file_dict[ability_types[i]]:
				wf.write(line.replace("*NAME*", name).replace("*EFFECT*", effect))
		print("    File Written")

with open('ability_file.txt', 'r') as rf:
	rf.readline()
	card_name = case(rf.readline().rstrip())
	ability_types = rf.readline().rstrip().split(' ')
	for i in range(len(ability_types)):
		ability_name = file_name(card_name, ability_types, i)
		ability_text = rf.readline().rstrip()
		write_file(ability_name, ability_text, ability_types, i)
